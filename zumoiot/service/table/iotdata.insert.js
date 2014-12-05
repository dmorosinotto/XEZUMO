function insert(item, user, request) {
    item.when = new Date(); //aggiungo la data nel log dei dati
    request.execute();
}