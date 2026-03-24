function iniciarFormularioTransacciones(urlObtenerCategorias) {
	$("#TipoOperacionId").on(async function () {
		const tipoOperacionId = $(this).val();
		const respuesta = await fetch(url, {
			method: 'POST',
			body: tipoOperacionId,
			headers: {
				'Content-Type': 'application/json'

			}
		});

		const json = await respuesta.json();
		console.log(json);
		const opciones = json.map(categoria => `<option value="${categoria.value}">${categoria.text}</option>`);
		$("#CategoriaId").html(opciones);
	})
}