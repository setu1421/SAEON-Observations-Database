e:
cd 'D:\Development\SAEON Observations Database\Ext.Net 1.2.0.21945'
[System.Reflection.Assembly]::Load("System.EnterpriseServices, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")            
$publish = New-Object System.EnterpriseServices.Internal.Publish            
$publish.GacInstall(".\Newtonsoft.Json.dll") 