function del(id, user, request) {
    console.warn({deleted: id, by: user.userId});
    request.execute();
}