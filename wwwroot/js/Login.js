document.getElementById("Registrar").addEventListener("click", function () {
    $('#myform').show();
});
    $("document").ready(function () {
        var body = document.body;
        var html = document.documentElement;
        var height = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
        /*var restar = document.getElementById("header").getBoundingClientRect().height;*/
        //height = height - restar;
        //restar = document.getElementById("footer").getBoundingClientRect().height;
        //height = height - restar;
        document.getElementById("Contenedor").style.height = height.toString() + "px";
    });

document.getElementById("login").addEventListener("click", Login);

function Login() {
    var pwd = JSON.stringify(document.getElementById('pwd').value);
    var _email = JSON.stringify(document.getElementById('email').value);


    $.ajax({
        type: 'GET',
        url: '?handler=Login',
        async: false,
        method: 'GET',
        headers: { 'Content-Type': 'application/json; charset=utf-8', 'Accept': '*/*' },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("RequestVerificationToken",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        dataType: 'html',
        data: ({
            email: _email,
            pwd: pwd
        }),
        cache: false,
        traditional: true,
        contentType: 'application/json',
        success: function (response) {
            var obj = JSON.parse(response);
            if (obj.Resultado == true) {
                showToast(obj.Respuesta);
                location.replace("./Principal");
            }
            else {
                showToast(obj.Respuesta);
            }
        }
    })
};

    const inputs = document.querySelectorAll('.fake_placeholder input');

    inputs.forEach(input => {
        //cuando entramos en el input
        input.onfocus = () => {
            //al elemento anterior (el span) le agregamos la clase que la reubica en top
            input.previousElementSibling.classList.add('reubicar');
        }

        //cuando salimos del input
        input.onblur = () => {
            //si no hay texto, le quitamos la clase reubicar,
            //para que se superponga con el input
            if (input.value.trim().length == 0)
                input.previousElementSibling.classList.remove('reubicar');
        }
    });

    function RegistrarUsuario() {
        var pwdNueva = JSON.stringify(document.getElementById('pwdNueva').value);
        var pwdNuevaRepetida = JSON.stringify(document.getElementById('pwdNuevaRepetida').value);
        var _nombre = JSON.stringify(document.getElementById('nombre').value);
        var _email = JSON.stringify(document.getElementById('emailNuevo').value);

        $.ajax({
            type: 'GET',
            url: '?handler=Registro',
            async: false,
            method: 'GET',
            headers: { 'Content-Type': 'application/json; charset=utf-8', 'Accept': '*/*' },
            beforeSend: function (xhr) {
                xhr.setRequestHeader("RequestVerificationToken",
                    $('input:hidden[name="__RequestVerificationToken"]').val());
            },
            dataType: 'html',
            data: ({
                nombre: _nombre,
                email: _email,
                newPWD: pwdNueva,
                pwdConfirm: pwdNuevaRepetida
            }),
            cache: false,
            traditional: true,
            contentType: 'application/json',
            success: function (response) {
                var obj = JSON.parse(response);
                if (obj.Resultado == true) {
                    showToast(obj.Respuesta);
                }
                else {
                    showToast(obj.Respuesta);
                }
            }
        })
};