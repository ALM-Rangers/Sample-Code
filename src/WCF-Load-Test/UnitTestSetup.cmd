rem This script must be run as an administrator to set up the HTTP API so that the test service used in some unit
rem tests does not need to be run as an administrator. The script uses netsh which means it can only be used on
rem Windows Vista or later. The alternative on other platforms is to use the httpcfg tool.

netsh http add urlacl url=http://+:8081/ user=everyone