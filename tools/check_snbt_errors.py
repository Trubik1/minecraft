# -*- coding: utf-8 -*-
with open(r"D:\SteampunkLauncher\logs\latest.log", "r", encoding="utf-8", errors="replace") as f:
    for line in f:
        if "Failed to read" in line or "can't read lang file" in line:
            print(line.strip())
