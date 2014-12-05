function sendpush() {
    var sql = "SELECT max(temp) as tmax, min(temp) as tmin, avg(umid) as umed, max(hwid) as name FROM IOTData WHERE __deleted=0";
    mssql.query(sql, {
        success: function (data) {
            if (data && data.length>0 && data[0].name) {
                push.send("ALL",data[0]);
                //pushHub(data[0],"ALL");
            } else {
                pushWNS("NO DATA","WP");
            }
        },
        error: function (err) {
            console.error(err);
            pushWNS("ERRORE SQL","UNIVERSAL");
        }
    }); 
}

// Invio di notifica push usando il provider nativo WNS per windows + eventuali Tags
function pushWNS(text,tags) {
    var payload = '<?xml version="1.0" encoding="utf-8"?><toast><visual><binding template="ToastText01">' +
        '<text id="1">' + text + '</text></binding></visual></toast>';
    push.wns.send(tags || null,
        payload,
        'wns/toast', {
            success: function (pushResponse) {
                console.log("Sent push: %s tags %s - ", text, tags, pushResponse );
            }
        });
}

// Invio di notifica push usando Notification Hub con payload cross-platform usando Template + Tags
function pushHub(payload, tags) {
    var azure = require('azure');
    var hub = azure.createNotificationHubService(process.env.MS_NotificationHubName, process.env.MS_NotificationHubConnectionString);
    hub.send(tags || null, payload, function(error) {
       if (error) console.error("ERROR PUSH HUB",error);
       else console.warn("OK Push Hub!", tags, payload); 
    });
}