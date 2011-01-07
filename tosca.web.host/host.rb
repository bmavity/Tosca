require 'rubygems'
require 'sqlite3'

dbfile = '../tosca.web/db/development.sqlite3'
db = SQLite3::Database.new dbfile

sql = <<SQL
  update reservations
  set confirmed = 'f'
  where id = ?
SQL

db.execute(sql, 2)


