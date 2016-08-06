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

    $scope.ValidaLancamento = function (modelo) {
        if (modelo.TipoD == null && modelo.TipoC == null) {
            $.notify({
                message: "Informe o Tipo de Lançamento"
            }, {
                type: "danger",
                delay: 2000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            });
            return false;
        }
        if (modelo.IdCategoria == null) {
            $.notify({
                message: "Informe uma Categoria"
            }, {
                type: "danger",
                delay: 2000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            });
            return false;
        }
        if (modelo.Descricao == null) {
            $.notify({
                message: "Informe uma Descrição"
            }, {
                type: "danger",
                delay: 2000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            });
            return false;
        }
        if (modelo.DataProcessamento == '') {
            $.notify({
                message: "Informe a Data do Lançamento"
            }, {
                type: "danger",
                delay: 2000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            });
            return false;
        }
        if (modelo.Valor == null) {
            $.notify({
                message: "Informa o Valor do Lançamento"
            }, {
                type: "danger",
                delay: 2000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            });
            return false;
        }
        if (modelo.Repeticao == null) {
            $.notify({
                message: "Informe a Repetição desse Lançamento"
            }, {
                type: "danger",
                delay: 2000,
                placement: {
                    from: "bottom",
                    align: "right"
                }
            });
            return false;
        }
        if ((modelo.Repeticao == '3') || (modelo.Repeticao == '4')) {
            if ((modelo.Vezes == null) || (modelo.Vezes == '0')) {
                $.notify({
                    message: "Informe a quantidade de repetições do Lançamento"
                }, {
                    type: "danger",
                    delay: 2000,
                    placement: {
                        from: "bottom",
                        align: "right"
                    }
                });
                return false;
            }
        }
        //-- Lançamento OK -->
        return true;
    }

    //chama o método IncluirLancamento do controlador
    $scope.AddLancamento = function (lancamento) {
        lancamento.IdConta = $("#IdConta").val();
        lancamento.TipoLancamento = $("#TipoLancamento:checked").val();
        lancamento.DataProcessamento = $("#DataProcessamento").val();
        if ($scope.ValidaLancamento(lancamento)) {
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
            });
        }
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
