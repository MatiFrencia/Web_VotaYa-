function irAEvento(evento)
    {
    var codigo = evento.dataset.codevento;
    window.location.replace("./Eventos?cod_ev=" + codigo);
    }