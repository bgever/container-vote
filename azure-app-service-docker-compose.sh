#!/bin/bash

dcao-merge -o azure-app-service-docker-compose.yml \
  docker-compose.yml \
  docker-compose.traefik-file.yml \
  docker-compose.azure-app-service.yml