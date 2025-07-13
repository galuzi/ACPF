using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using ACPF.DataAccess;

namespace ACPF.UI.ViewModels
{
    /// <summary>
    /// ViewModel principal da aplicação
    /// </summary>
    public class MainViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private BaseViewModel? _currentViewModel;
        private decimal _saldoTotal;
        private decimal _totalReceitas;
        private decimal _totalDespesas;
        private bool _isTransacoesAtivo;
        private bool _isCategoriasAtivo;
        private bool _isRelatoriosAtivo;

        public MainViewModel(AppDbContext context)
        {
            _context = context;
            
            // Inicializar ViewModels das abas
            TransacaoViewModel = new TransacaoViewModel(context);
            CategoriaViewModel = new CategoriaViewModel(context);
            RelatorioViewModel = new RelatorioViewModel(context);

            // Definir ViewModel inicial
            CurrentViewModel = TransacaoViewModel;
            IsTransacoesAtivo = true;

            // Carregar dados iniciais
            CarregarDados();
        }

        /// <summary>
        /// ViewModel atual sendo exibido
        /// </summary>
        public BaseViewModel? CurrentViewModel
        {
            get => _currentViewModel;
            set => SetProperty(ref _currentViewModel, value);
        }

        /// <summary>
        /// ViewModel para gerenciamento de transações
        /// </summary>
        public TransacaoViewModel TransacaoViewModel { get; }

        /// <summary>
        /// ViewModel para gerenciamento de categorias
        /// </summary>
        public CategoriaViewModel CategoriaViewModel { get; }

        /// <summary>
        /// ViewModel para relatórios
        /// </summary>
        public RelatorioViewModel RelatorioViewModel { get; }

        /// <summary>
        /// Saldo total (receitas - despesas)
        /// </summary>
        public decimal SaldoTotal
        {
            get => _saldoTotal;
            set => SetProperty(ref _saldoTotal, value);
        }

        /// <summary>
        /// Total de receitas
        /// </summary>
        public decimal TotalReceitas
        {
            get => _totalReceitas;
            set => SetProperty(ref _totalReceitas, value);
        }

        /// <summary>
        /// Total de despesas
        /// </summary>
        public decimal TotalDespesas
        {
            get => _totalDespesas;
            set => SetProperty(ref _totalDespesas, value);
        }

        /// <summary>
        /// Indica se a aba de transações está ativa
        /// </summary>
        public bool IsTransacoesAtivo
        {
            get => _isTransacoesAtivo;
            set => SetProperty(ref _isTransacoesAtivo, value);
        }

        /// <summary>
        /// Indica se a aba de categorias está ativa
        /// </summary>
        public bool IsCategoriasAtivo
        {
            get => _isCategoriasAtivo;
            set => SetProperty(ref _isCategoriasAtivo, value);
        }

        /// <summary>
        /// Indica se a aba de relatórios está ativa
        /// </summary>
        public bool IsRelatoriosAtivo
        {
            get => _isRelatoriosAtivo;
            set => SetProperty(ref _isRelatoriosAtivo, value);
        }

        /// <summary>
        /// Comando para navegar para a aba de transações
        /// </summary>
        public RelayCommand NavegarParaTransacoesCommand => new RelayCommand(() =>
        {
            CurrentViewModel = TransacaoViewModel;
            IsTransacoesAtivo = true;
            IsCategoriasAtivo = false;
            IsRelatoriosAtivo = false;
        });

        /// <summary>
        /// Comando para navegar para a aba de categorias
        /// </summary>
        public RelayCommand NavegarParaCategoriasCommand => new RelayCommand(() =>
        {
            CurrentViewModel = CategoriaViewModel;
            IsTransacoesAtivo = false;
            IsCategoriasAtivo = true;
            IsRelatoriosAtivo = false;
        });

        /// <summary>
        /// Comando para navegar para a aba de relatórios
        /// </summary>
        public RelayCommand NavegarParaRelatoriosCommand => new RelayCommand(() =>
        {
            CurrentViewModel = RelatorioViewModel;
            IsTransacoesAtivo = false;
            IsCategoriasAtivo = false;
            IsRelatoriosAtivo = true;
        });

        /// <summary>
        /// Carrega os dados iniciais da aplicação
        /// </summary>
        public async void CarregarDados()
        {
            try
            {
                // Calcular totais usando LINQ
                var receitas = await _context.Transacoes
                    .Where(t => t.Tipo == ACPF.Core.Enums.TipoTransacao.Receita)
                    .SumAsync(t => t.Valor);

                var despesas = await _context.Transacoes
                    .Where(t => t.Tipo == ACPF.Core.Enums.TipoTransacao.Despesa)
                    .SumAsync(t => t.Valor);

                TotalReceitas = receitas;
                TotalDespesas = despesas;
                SaldoTotal = receitas - despesas;

                // Notificar ViewModels filhos para recarregar dados
                TransacaoViewModel.CarregarTransacoes();
                CategoriaViewModel.CarregarCategorias();
                RelatorioViewModel.CarregarRelatorios();
            }
            catch (Exception ex)
            {
                // Em uma aplicação real, você usaria um sistema de logging
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar dados: {ex.Message}");
            }
        }

        /// <summary>
        /// Atualiza os totais quando uma transação é adicionada ou removida
        /// </summary>
        public void AtualizarTotais()
        {
            CarregarDados();
        }
    }
} 