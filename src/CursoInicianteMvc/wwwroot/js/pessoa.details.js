function btnAcoes(id, row){
    const urlDetalhes = $("input#url-tarefa-detalhes");

    let html = `<a href="${urlDetalhes.val().concat("/").concat(id)}" class="btn btn-primary">Detalhes</a>`

    if (!row.realizadoEm)
    {
        const urlEditar = $("input#url-tarefa-editar");
        const urlExcluir = $("input#url-tarefa-excluir");
        html += `
                <a href="${urlEditar.val().concat("/").concat(id)}" class="btn btn-info">Editar</a>
                <a href="${urlExcluir.val().concat("/").concat(id)}" class="btn btn-danger">Excluir</a>
            `
    }

    return html;
}

function formatarData(id){
    if (id)
        return new Date(id).toLocaleDateString();
    return "";
}