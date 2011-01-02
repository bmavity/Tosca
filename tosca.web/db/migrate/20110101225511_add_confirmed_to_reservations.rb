class AddConfirmedToReservations < ActiveRecord::Migration
  def self.up
    add_column :reservations, :confirmed, :bool
  end

  def self.down
    remove_column :reservations, :confirmed
  end
end
