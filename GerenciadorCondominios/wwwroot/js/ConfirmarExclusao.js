function ConfirmarExclusao(id, nome, controller) {
    //inicializar todos modais
    $('.modal').modal({
        dismissble: true
    });

    //abrir modal
    $('#modal').modal('open');
    $(".nome").text(nome);
    //montando url
    const url = "/" + controller + "/Delete"

    //clicando no botão excluir
    $(".btnExcluir").on('click', function () {

        //chamada ajax
        $.ajax({
            method: "POST",
            url: url,
            data: { id: id },
            success: function (data) {
                location.reload(true);
            }
        })
    })
}