require 'rake'

Gem::Specification.new do |s|
  s.platform    = Gem::Platform::RUBY
  s.name        = 'tosca-messages'
  s.version     = '0.0'
  
  s.summary     = 'messages'
  s.description = 'messages'
  
  s.author            = 'ACM,PBG,LEGO'
  s.email             = 'dru@drusellers.com'
  s.homepage          = 'http://www.masstransit.com'
  s.rubyforge_project = 'masstransit'

  s.files = Dir['lib/*.rb']
  #s.bindir             = 'bin'
  #s.executables        = ['mt']
  #s.default_executable = 'mt'
end
