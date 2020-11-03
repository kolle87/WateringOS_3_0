#!/usr/bin/env python3
import paho.mqtt.publish as publish
import sys

param = sys.argv.copy()
param.pop(0) # remove script name

client_id  = str(param[0])
param.pop(0)
sensor_id  = str(param[0])
param.pop(0)

vtopic = "/v1.0/%s/sensor/%s/metric/pushValues" % (client_id, sensor_id)

vpayload = "{ \"metrics\": [ "

while len(param) > 0:
	vpayload = vpayload + "{\"value\" : %s, \"metricId\" : \"%s\" }," % (param[0], param[1])
	param.pop(0)
	param.pop(0)

vpayload = vpayload[:-1] + " ] }"

print (vpayload)

publish.single(topic=vtopic, payload=vpayload, hostname="localhost", port=18884, tls={'ca_certs':"/var/lib/teamviewer-iot-agent/certs/TeamViewerAuthority.crt",'certfile':"/home/pi/clientCert.crt",'keyfile':"/home/pi/privkey.pem"})