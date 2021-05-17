#!/bin/bash
docker stop payment-tracker-db
docker rm payment-tracker-db 
docker-compose up -d
