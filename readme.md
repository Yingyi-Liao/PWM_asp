Password Manager written with ASP.NET extended with RESTful API

need to configure appsettings.json to function
where you need to use your own gmail(or other smtp service provider) smtp service to complete the register verification

https implementation is optional
if you want to save some headache for certificate configuration, just remove https parts in appsetting.json
otherwise export certificate from certlm.msc -> Personal -> Certificates -> https certificate issued to yout domain or machine name
choose:
  yes, export the private key with format PFX
Check:
  Include all certificates in the certification path
  Enable certificate privacy
Set a password
Save the .pfx file as cert.pfx to the PWM_asp folder

suggest using with published version with  appsetting.json production in production environment
while using in production/development environment
setting environment variable PWM_MASTER_KEY with base64 format is required for encryption master key
