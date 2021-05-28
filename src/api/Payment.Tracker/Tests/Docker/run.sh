#!/bin/bash
set -e

docker-compose -f $BUILD_SOURCESDIRECTORY/src/api/Payment.Tracker/Tests/Docker/docker-compose.yml -p bartekzawadka/payment-tracker rm
docker-compose -f $BUILD_SOURCESDIRECTORY/src/api/Payment.Tracker/Tests/Docker/docker-compose.yml -p bartekzawadka/payment-tracker build
docker-compose -f $BUILD_SOURCESDIRECTORY/src/api/Payment.Tracker/Tests/Docker/docker-compose.yml -p bartekzawadka/payment-tracker up --force-recreate --abort-on-container-exit
docker cp payment-tracker-integration-tests:/app/Tests/Payment.Tracker/Tests/TestResults/TestResults.trx $BUILD_SOURCESDIRECTORY