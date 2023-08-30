function btnAcoes(id){
    const urlDetalhes = $("input#url-detalhes").val();
    return `<a href="${urlDetalhes.concat("/").concat(id)}" class="btn btn-primary">Detalhes</a>`
}