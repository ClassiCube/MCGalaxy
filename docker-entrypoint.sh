#!/usr/bin/env sh

set -e

mkdir -p ${DATA_DIR}
cd ${DATA_DIR}
touch MCGalaxy.db
mkdir -p blockdb \
  blockdefs \
  backups \
  bots \
  extra \
  levels \
  logs \
  players \
  properties \
  ranks \
  ranksbackup \
  text
cd ${MCGALAXY}
ln -sf ${DATA_DIR}/blockdb blockdb
ln -sf ${DATA_DIR}/blockdefs blockdefs
ln -sf ${DATA_DIR}/backups backups
ln -sf ${DATA_DIR}/bots bots
ln -sf ${DATA_DIR}/extra extra
ln -sf ${DATA_DIR}/levels levels
ln -sf ${DATA_DIR}/logs logs
ln -sf ${DATA_DIR}/players players
ln -sf ${DATA_DIR}/properties properties
ln -sf ${DATA_DIR}/ranks ranks
ln -sf ${DATA_DIR}/ranksbackup ranksbackup
ln -sf ${DATA_DIR}/text text
ln -sf ${DATA_DIR}/MCGalaxy.db MCGalaxy.db

exec "$@"
