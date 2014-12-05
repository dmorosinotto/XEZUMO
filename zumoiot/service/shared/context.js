//Init context only once
var _context = { app: null, io: null };

//and Save it from startup
exports.setContext = function(context) {	
	for (var k in context) {
		_context[k] = context[k];
	}
}

exports.getContext = function() { return _context; }

exports.getExpressApp = function() { return _context.app; }

exports.getSocketIo = function() { return _context.io; }