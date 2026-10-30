[Net.ServicePointManager]::SecurityProtocol = [Net.SecurityProtocolType]::Tls12
$url = 'https://api.adoptium.net/v3/binary/latest/21/ga/windows/x64/jre/hotspot/normal/eclipse'
$req = [System.Net.HttpWebRequest]::Create($url)
$req.AllowAutoRedirect = $true
$resp = $req.GetResponse()
Write-Host "Adoptium JRE URL:" $resp.ResponseUri.AbsoluteUri
Write-Host "Content Length:" $resp.ContentLength
$resp.Close()
