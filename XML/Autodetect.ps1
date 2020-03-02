param (
[Parameter(Mandatory=$true)][string]$binfile
)

# Read the entire file to an array of bytes.
$bytes = [System.IO.File]::ReadAllBytes($binfile)

# Decode first 12 bytes to a text assuming ASCII encoding.
$text = [System.Text.Encoding]::ASCII.GetString($bytes, 0, 12)

#Read Checksum from $500
$CSAddress = 0x500
$CSSize = 2
$CSBytes = $bytes[($CSAddress+$SCSize-1)..$CSAddress]
$checksum =[bitconverter]::ToUInt16($CSBytes,0)
$Result = [String]::Format("{0:x}", $checksum )

if ((get-item $binfile).length -eq 512kB)
{
	write-output "v6.xml"
}
if ((get-item $binfile).length -eq 1Mb)
{
	write-output "P01-P59.xml"
}
if ((get-item $binfile).length -eq 2Mb)
{
	write-output "e38.xml"
}