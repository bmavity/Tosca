require 'masstransit'

class ApplicationController < ActionController::Base
  protect_from_forgery
  
  
  def get_bus()
    conf = MassTransit.load_config('./sample_publisher.yaml')
    bus = MassTransit::Bus.new(conf)
  end
  
  def get_message(reservation)
    msg = CreateReservation.new
    msg.name = reservation.name
    msg.number = reservation.number
    
    msg
  end
end
