function insert(item, user, request) {
    item.when = new Date(); //aggiungo la data nel log dei dati
    request.execute({
    	success: function() {
    		//Ricavo il contesto di Socket.IO per inviare notifica logiot del dato inserito
    		var io = require('../shared/context').getSocketIo();
    		io.emit('logiot', item); //qui item dovrebbe avere anche Id
    		request.respond();	
    	}
    });
}