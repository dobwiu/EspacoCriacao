//define aplicação angular e o controller
var app = angular.module("orcamentoApp", []);

//registra o controller e cria a função para obter os dados do Controlador MVC
app.controller("financasCtrl", function ($scope, $http) {
    var idConta = $("#IdConta").val();
    $http.get('/Financas/Lancamentos/GetLancamentos/' + idConta)
    .success(function (result) {
        $scope.extrato = result;
        $scope.novoLanc.Repeticao = '0';
    })
    .error(function (data) {
        console.log(data);
    });

    //chama o método IncluirLancamento do controlador
    $scope.AddLancamento = function (lancamento) {
        lancamento.IdConta = $("#IdConta").val();
        lancamento.TipoLancamento = $("#TipoLancamento:checked").val();
        lancamento.DataProcessamento = $("#DataProcessamento").val();
        $http.post('/Financas/Lancamentos/AddLancamento/', { addLancamento: lancamento })
        .success(function (result) {
            $scope.extrato = result;
            delete $scope.novoLanc;
            $("#DataProcessamento").val('');
            $.notify({
                message: "Registro salvo com sucesso"
            }, {
                type: "success",
                delay: 1000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            }
            );
        })
        .error(function (data) {
            console.log(data);
            $.notify({
                message: data
            }, {
                type: "danger",
                delay: 7500,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            }
            );
        });
    }

    //chama o método ExcluirLancamento do controlador
    $scope.DelLancamento = function (lancamento) {
        $http.post('/Financas/Lancamentos/DelLancamento/', { delLancamento: lancamento })
        .success(function (result) {
            $scope.extrato = result;
            $("#DataProcessamento").val('');
            $.notify({
                message: "Registro excluído com sucesso"
            }, {
                type: "success",
                delay: 1000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            }
            );
        })
        .error(function (data) {
            console.log(data);
            $.notify({
                message: data
            }, {
                type: "danger",
                delay: 7500,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            }
            );
        });
    }

    //carrega as categorias do tipo de lançamento selecionado
    $scope.CarregaComboCategoria = function (opt) {
        $scope.novoLanc.TipoLancamento = opt;
        $http.post('/Financas/Lancamentos/ComboCategorias', { tipo: opt })
        .success(function (result) {
            $scope.ListaCategorias = result;
        }).error(function (data) {
            console.log(data);
        });
    }
})
