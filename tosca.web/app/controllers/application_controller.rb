class ApplicationController < ActionController::Base
  protect_from_forgery
  
  
  def get_bus()
    file_path = File.expand_path('./config/bus.yaml', Rails.root)
    conf = MassTransit.load_config(file_path)
    bus = MassTransit::Bus.new(conf)
  end
  
  def get_message(reservation)
    msg = Tosca::Messages::CreateReservation.new
    msg.Name = reservation.name
    msg.Number = reservation.number_in_party
    
    msg
  end
end

module Tosca
  module Messages
    class CreateReservation
      attr_accessor :Name
      attr_accessor :Number
      
    end
  end
end
