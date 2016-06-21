//define aplicação angular e o controller
var app = angular.module("orcamentoApp", []);

//registra o controller e cria a função para obter os dados do Controlador MVC
app.controller("ambienteCtrl", function ($scope, $http) {
    var idPedido = $("#IdPedido").val();
    $http.get('/Orc/Pedidos/GetAmbientes/' + idPedido)
    .success(function (result) {
        $scope.ambientes = result;
    })
    .error(function (data) {
        console.log(data);
    });

    //chama o  método IncluirAmbiente do controlador
    $scope.AddAmbiente = function (ambiente) {
        ambiente.IdPedido = $("#IdPedido").val();
        $http.post('/Orc/Pedidos/IncluirAmbiente/', { novoAmbiente: ambiente })
        .success(function (result) {
            $scope.ambientes = result;
            delete $scope.novoAmb;
        })
        .error(function (data) {
            console.log(data);
        });
    }

    //chama o  método ExcluirAmbiente do controlador
    $scope.DeletaAmbiente = function (ambiente) {
        $http.post('/Orc/Pedidos/ExcluirAmbiente/', { delAmbiente: ambiente })
        .success(function (result) {
            $scope.ambientes = result;
        })
        .error(function (data) {
            console.log(data);
        });
    }
})

//registra o controller e cria a função para obter os dados do Controlador MVC
app.controller("parcelasCtrl", function ($scope, $http) {
    var idPedido = $("#IdPedido").val();
    $http.get('/Orc/Pedidos/GetParcelas/' + idPedido)
    .success(function (result) {
        $scope.parcelas = result;
    })
    .error(function (data) {
        console.log(data);
    });

    //chama o  método IncluirParcela do controlador
    $scope.AddParcela = function (parcela) {
        parcela.IdPedido = $("#IdPedido").val();
        parcela.IdCliente = $("#IdCliente").val();
        $http.post('/Orc/Pedidos/IncluirParcela/', { novaParcela: parcela })
        .success(function (result) {
            $scope.parcelas = result;
            delete $scope.novaParc;
            $("#parcDataPagto").val('');
        })
        .error(function (data) {
            console.log(data);
        });
    }

    //chama o  método ExcluirParcela do controlador
    $scope.DeletaParcela = function (parcela) {
        $http.post('/Orc/Pedidos/ExcluirParcela/', { delParcela: parcela })
        .success(function (result) {
            $scope.parcelas = result;
        })
        .error(function (data) {
            console.log(data);
        });
    }
})