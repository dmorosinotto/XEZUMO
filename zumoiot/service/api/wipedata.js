// Use "request.service" to access features of your mobile service, e.g.:
//   var tables = request.service.tables;
//   var push = request.service.push;
exports.post = function(request, response) {
    sendPush(request,'Yes Sir!');
    response.send(statusCodes.OK, { message : 'Yes sir! (only Admin)' });
};

exports.get = function(request, response) {
    var sql = "SELECT hwid, temp, umid FROM IOTData";
    if (request.user.level !== 'admin') sql += " WHERE __deleted=1";
    
    var mssql = request.service.mssql;
    mssql.query(sql, {
        success: function (results) {
            response.send(statusCodes.OK, results);
        },
        error: function (err) {
            console.error(err);
            response.send(statusCodes.INTERNAL_SERVER_ERROR, err);
        }
    });
};

exports.delete = function(request, response) {
    if ((request.user.level == 'admin') || (request.user.userId.match(/^aad:/gi))) {
        var sql = "DELETE FROM IOTData";
        if (request.user.level !== 'admin') sql += " WHERE __deleted=1";
        
        var mssql = request.service.mssql;
        mssql.query(sql, {
            success: function (res) {
                var msg = (request.user.level == 'admin' ? "Wiped ALL" : "Clear deleted") + " data!";
                sendPush(request, msg);
                response.send(statusCodes.OK, msg);
            },
            error: function (err) {
                console.error(err);
                response.send(statusCodes.INTERNAL_SERVER_ERROR, err);
            }
        });    
    } else {
        response.send(statusCodes.FORBIDDEN, "Only Admin or AAD Users can delete data!");    
    }   
};

function sendPush(request,msg) {
    var push = request.service.push;
    push.wns.send(null,'<?xml version="1.0" encoding="utf-8"?><toast><visual><binding template="ToastText01"><text id="1">' + 
         msg + '</text></binding></visual></toast>','wns/toast');
}