Password Manager written with ASP.NET extended with RESTful API<br>
<br>
need to configure appsettings.json to function<br>
where you need to use your own gmail(or other smtp service provider) smtp service to complete the register verification<br>
<br>
https implementation is optional<br>
if you want to save some headache for certificate configuration, just remove https parts in appsettings.json<br>
However, without https, decrypted password cannot be copied to another machine for obvious reason<br>
otherwise export certificate from certlm.msc -> Personal -> Certificates -> https certificate issued to yout domain or machine name<br>
choose:<br>
  yes, export the private key with format PFX<br>
Check:<br>
  Include all certificates in the certification path<br>
  Enable certificate privacy<br>
Set a password<br>
Save the .pfx file as cert.pfx to the PWM_asp folder<br>
<br>
suggest using with published version with  appsetting.json production in production environment<br>
while using in production/development environment<br>
setting environment variable PWM_MASTER_KEY with base64 format is required for encryption master key<br>
<br>
WARNING:<br>
for safe hosting make sure using encrypted providers with https<br>
if hosting in local environment, make sure all devices are trusted and firewall ready<br>
