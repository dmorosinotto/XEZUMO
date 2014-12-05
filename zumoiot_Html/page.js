$(function() {
    var client = new WindowsAzure.MobileServiceClient('https://<SCRIVERE_QUI_NOMESERVIZIO>.azure-mobile.net/', '<SCRIVERE_QUI_APPLICATIONKEY>'),
        todoItemTable = client.getTable('iotdata');

    // Read current data and rebuild UI.
    // If you plan to generate complex UIs like this, consider using a JavaScript templating library.
    function refreshTodoItems() {
        var query = todoItemTable.orderByDescending("when");

        query.read().then(function(todoItems) {
            var listItems = $.map(todoItems, function(item) {
                return $('<li>')
                    .attr('data-todoitem-id', item.id)
                    .append($('<button class="item-delete">Delete</button>'))
                    .append($('<input type="checkbox" class="item-complete">'))
                    .append($('<span>').append($('<input class="item-text">').val(item.hwid)))
                    .append($('<div>').text(item.temp))
                    .append($('<div>').text(item.umid))
                    .append($('<span>').text(item.when && item.when.toISOString()));
            });

            $('#todo-items').empty().append(listItems).toggle(listItems.length > 0);
            $('#summary').html('<strong>' + todoItems.length + '</strong> item(s)');
        }, handleError);
    }

    function handleError(error) {
        var text = error + (error.request ? ' - ' + error.request.status : '');
        $('#errorlog').append($('<li>').text(text));
    }

    function getTodoItemId(formElement) {
        return $(formElement).closest('li').attr('data-todoitem-id');
    }

    function getRnd(min, max) {
        return Math.floor(Math.random() * (max-min))+min;
    }

    // Handle insert
    $('#add-item').submit(function(evt) {
        var textbox = $('#new-item-text'),
            itemText = textbox.val();
        if (itemText !== '') {
            todoItemTable.insert({ hwid: itemText, temp: getRnd(10,30), umid: getRnd(50,100)}).then(refreshTodoItems, handleError);
        }
        textbox.val('').focus();
        evt.preventDefault();
    });

    // Handle update
    $(document.body).on('change', '.item-text', function() {
        var newText = $(this).val();
        todoItemTable.update({ id: getTodoItemId(this), hwid: newText }).then(null, handleError);
    });

    $(document.body).on('change', '.item-complete', function() {
        var isComplete = $(this).prop('checked');
        if (isComplete) 
            todoItemTable.update({ id: getTodoItemId(this), temp: getRnd(-20,0), umid: getRnd(90,150) }).then(refreshTodoItems, handleError);
    });

    // Handle delete
    $(document.body).on('click', '.item-delete', function () {
        todoItemTable.del({ id: getTodoItemId(this) }).then(refreshTodoItems, handleError);
    });

    $("#refresh").on('click',refreshTodoItems);

    $("#login").on('click', function() {
        var isLoggedIn = client.currentUser !== null;
        if (isLoggedIn) {
            client.logout();
            $("#login").text("Log In");
        } else {
            var provider = prompt("Effettuare Login con il provider:","aad") || 'aad';
            client.login(provider).then(
                function () { alert ("Welcome " + client.currentUser.userId); },
                function (error) { alert ("Errore Login: " + error); }
            );
            $("#login").text("Log Out");
        }
    });

    // On initial load, start by fetching the current data
    refreshTodoItems();
});