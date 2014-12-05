function insert(item, user, request) {
    //semplice logica di validazione prima di inserire    
    if (item.text && item.text.length>3) {
        request.execute();
    } else {
        request.response(statusCodes.BAD_REQUEST, {error: 'Specificare un testo di almeno 3 caratteri!'});
    }
}