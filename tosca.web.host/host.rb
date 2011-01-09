require 'rubygems'
require 'sqlite3'
require '../lib/masstransit'

def process_message(msg)
  dbfile = '../tosca.web/db/development.sqlite3'
  db = SQLite3::Database.new dbfile

sql = <<SQL
  update reservations
  set confirmed = 'f'
  where id = ?
SQL

  db.execute(sql, msg.id)
end



conf = MassTransit.load_config('./host.yaml')
bus = MassTransit::Bus.new(conf)

#subscribe stuff
bus.subscribe('MyMessage') do |msg|
  process_message msg
end

bus.start()