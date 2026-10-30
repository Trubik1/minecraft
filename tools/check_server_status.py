import socket
import time
import json
import struct
import sys

def ping_server(host, port=25565, timeout=5.0):
    print(f"Connecting to {host}:{port}...")
    start_time = time.time()
    try:
        s = socket.create_connection((host, int(port)), timeout=timeout)
        tcp_handshake_ms = (time.time() - start_time) * 1000
        
        # Minecraft handshake packet
        # Packet ID: 0x00
        # Protocol: -1 (or 767 for 1.21.1)
        # Host: string
        # Port: unsigned short
        # Next state: 1 (status)
        host_bytes = host.encode('utf-8')
        
        # Helper varint
        def encode_varint(val):
            out = bytearray()
            while True:
                part = val & 0x7F
                val >>= 7
                if val:
                    out.append(part | 0x80)
                else:
                    out.append(part)
                    break
            return bytes(out)
        
        handshake_data = b'\x00' + encode_varint(767) + encode_varint(len(host_bytes)) + host_bytes + struct.pack('>H', int(port)) + b'\x01'
        s.sendall(encode_varint(len(handshake_data)) + handshake_data)
        
        # Status request packet: length 1, id 0x00
        s.sendall(b'\x01\x00')
        
        # Read response
        ping_start = time.time()
        
        def read_varint():
            res = 0
            shift = 0
            while True:
                b = s.recv(1)
                if not b:
                    raise Exception("Connection closed")
                val = b[0]
                res |= (val & 0x7F) << shift
                if not (val & 0x80):
                    break
                shift += 7
            return res

        packet_len = read_varint()
        packet_id = read_varint()
        data_len = read_varint()
        
        data = bytearray()
        while len(data) < data_len:
            chunk = s.recv(min(4096, data_len - len(data)))
            if not chunk:
                break
            data.extend(chunk)
            
        full_ping_ms = (time.time() - ping_start) * 1000
        s.close()
        
        info = json.loads(data.decode('utf-8'))
        print("\n================ SERVER STATUS ================")
        print(f"Status:      ONLINE")
        print(f"TCP Ping:    {tcp_handshake_ms:.1f} ms")
        print(f"Game Ping:   {full_ping_ms:.1f} ms")
        print(f"Version:     {info.get('version', {}).get('name')}")
        print(f"Players:     {info.get('players', {}).get('online')}/{info.get('players', {}).get('max')}")
        print(f"MOTD:        {info.get('description')}")
        print("===============================================\n")
        return True
    except Exception as e:
        print(f"\n[!] Failed to connect to server: {e}")
        return False

if __name__ == "__main__":
    host = sys.argv[1] if len(sys.argv) > 1 else "localhost"
    port = sys.argv[2] if len(sys.argv) > 2 else 25565
    ping_server(host, port)
