require 'rubygems'
require 'sqlite3'
require 'masstransit'

def process_message(msg)
  dbfile = '../tosca.web/db/development.sqlite3'
  db = SQLite3::Database.new dbfile

sql = <<SQL
  update reservations
  set confirmed = 't'
  where name = ?
SQL

  db.execute(sql, msg.Name)
end



conf = MassTransit.load_config('./host.yaml')
bus = MassTransit::Bus.new(conf)

#subscribe stuff
bus.subscribe('urn:messages:Tosca:Messages:ReservationConfirmed') do |msg|
  puts '111111111111'
  process_message msg
end

bus.subscribe('Tosca.Messages.ReservationConfirmed, Tosca.Messages') do |msg|
  puts '2222222222'
  process_message msg
end
bus.start()
