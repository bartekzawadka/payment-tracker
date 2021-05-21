#!/bin/bash
docker stop payment-tracker-db
docker rm payment-tracker-db 
docker-compose -f ../scripts/docker-compose.yaml up -d payment-tracker-db
