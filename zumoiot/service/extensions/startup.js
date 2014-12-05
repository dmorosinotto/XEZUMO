//Called after all route, so you can't set a Middleware!

exports.startup = function (context, done) {
	console.log('Init socket.io');
	var io = require('socket.io')(context.app.server);
	io.on('connection', function(socket){
	  //Gestisco ricezione remote cmd dal Monitor --> invio logiot per aggiornare UniversalAPP e echo su Monitor
	  socket.on('remote', function(msg){
		io.emit('logiot', msg);
	  });
	});	
  	
  	//Ritorno la pagina html del Monitor fatto con Socket.IO 
  	var path = require('path');
  	context.app.get('/monitor.html', function(req, res) {
		res.sendfile(path.resolve(__dirname, '../public/monitor.html'));
	});	

  	//Salvo il contesto Socket.IO per poterlo utilizzare da Insert function sulle tabelle
  	context.io = io;
  	require('../shared/context').setContext(context);
	done();
}