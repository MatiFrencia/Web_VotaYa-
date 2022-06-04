function irAEvento(evento)
    {
    var codigo = evento.dataset.codevento;
    window.location.replace("./Eventos?cod_ev=" + codigo);
}

function RegistrarEvento() {
    var Hour = document.querySelector('[title="Pick Hour"]').innerHTML;
    var Minutes = document.querySelector('[title="Pick Minute"]').innerHTML;
    if (Hour == "") {
        Hour = "vacio";
    }
    if (Minutes == "") {
        Minutes = "vacio";
    }
    var fecha = document.getElementsByClassName("day active")[0].dataset.value + " " + Hour + ":" + Minutes + ":00";
    var _nombre = document.getElementById('nombre').value;
    var _descripcion = document.getElementById("descripcion").value;

    $.ajax({
        type: 'GET',
        url: '?handler=RegistroEvento',
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
            FechaInicio: fecha,
            descripcion: _descripcion
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

function IngresarEvento() {
    var _Codigo = document.getElementById('Codigo').value;

    $.ajax({
        type: 'GET',
        url: '?handler=IngresarEvento',
        async: false,
        method: 'GET',
        headers: { 'Content-Type': 'application/json; charset=utf-8', 'Accept': '*/*' },
        beforeSend: function (xhr) {
            xhr.setRequestHeader("RequestVerificationToken",
                $('input:hidden[name="__RequestVerificationToken"]').val());
        },
        dataType: 'html',
        data: ({
            Codigo: _Codigo
        }),
        cache: false,
        traditional: true,
        contentType: 'application/json',
        success: function (response) {
            var obj = JSON.parse(response);
            if (obj.Resultado == true) {
                showToast(obj.Respuesta);
                window.location.reload();
            }
            else {
                showToast(obj.Respuesta);
            }
        }
    })
};