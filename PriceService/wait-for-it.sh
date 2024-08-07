#!/bin/bash
# wait-for-it.sh is a script to wait for a service to be ready

host="$1"
shift
port="$1"
shift

echo "Waiting for $host:$port..."

while ! nc -zv "$host" "$port"; do
  echo "$host:$port is not available yet..."
  sleep 2
done

echo "$host:$port is available!"

exec "$@"
