function update(item, user, request) {
    item.when = new Date(); //aggiorno la data nel record
    request.execute();
}