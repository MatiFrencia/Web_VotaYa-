var btnRegistrar = document.getElementById("Registrar");
if (btnRegistrar != null) {
    btnRegistrar.addEventListener("click", function () {
        $('#myform').show();
    });
}
    $("document").ready(function () {
        var body = document.body;
        var html = document.documentElement;
        var height = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
        /*var restar = document.getElementById("header").getBoundingClientRect().height;*/
        //height = height - restar;
        //restar = document.getElementById("footer").getBoundingClientRect().height;
        //height = height - restar;
        var btnContenedor = document.getElementById("Contenedor");
        if (btnContenedor != null) {
            btnContenedor.style.height = height.toString() + "px";
        }
    });

var btnLogin = document.getElementById("login");
if (btnLogin != null) {
    btnLogin.addEventListener("click", Login);
}

document.addEventListener('keyup', event => {
    if (event.keyCode === 13) {
        var modal_registro = document.getElementById("ModalRegistro");
        if (modal_registro.style.display != "none" && modal_registro.style.opacity != 0) {
            document.getElementById("GuardarUsuario").click();
        }
        else {
            document.getElementById("login").click();
        }
    }
}, false)

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