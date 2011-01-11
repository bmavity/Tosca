class Reservation < ActiveRecord::Base
  
  def after_initialize
    self.confirmed=false unless self.confirmed
  end
end
