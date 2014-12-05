exports.register = function (registration, registrationContext, done) {
    // Use this callback to check if the user is authorized to register for all the tags in registration.tags.
    // 
    // Use "registrationContext.service" to access features of your mobile service, e.g.:
    //   var tables = registrationContext.service.tables;
    //         
    // Use "registrationContext.user" to access user information, e.g.:
    //   var userId = registrationContext.user.userId;    
    //
    // You can also add or remove tags, example:
    //   registration.tags.push('myTag');
    //
    // Additional Information: http://go.microsoft.com/fwlink/?LinkId=400845
    var isWin = false;
    var tags = registration.tags || [];
    for (var i=0; i<tags.length; i++) {
        if (tags[i].match(/^w/gi)) isWin = true;
    }
    if (isWin && tags.indexOf('UNIVERSAL')==-1) tags.push('UNIVERSAL');
    if (tags.indexOf('ALL')==-1) tags.push('ALL');
    
    done();
};