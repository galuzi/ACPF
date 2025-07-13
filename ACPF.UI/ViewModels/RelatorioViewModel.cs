using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;
using ACPF.Core.Models;
using ACPF.Core.Enums;
using ACPF.DataAccess;

namespace ACPF.UI.ViewModels
{
    /// <summary>
    /// ViewModel para relatórios e análises financeiras
    /// </summary>
    public class RelatorioViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Transacao> _transacoes;
        private decimal _totalReceitas;
        private decimal _totalDespesas;
        private decimal _saldo;
        private DateTime _dataInicio = DateTime.Now.AddMonths(-1);
        private DateTime _dataFim = DateTime.Now;
        private Categoria? _categoriaFiltro;
        private ObservableCollection<Categoria> _categorias;
        private ObservableCollection<RelatorioPorCategoria> _relatorioPorCategoria;

        public RelatorioViewModel(AppDbContext context)
        {
            _context = context;
            _transacoes = new ObservableCollection<Transacao>();
            _categorias = new ObservableCollection<Categoria>();
            _relatorioPorCategoria = new ObservableCollection<RelatorioPorCategoria>();

            // Inicializar comandos
            GerarRelatorioCommand = new RelayCommand(GerarRelatorio);
            ExportarRelatorioCommand = new RelayCommand(ExportarRelatorio);

            // Carregar dados iniciais
            CarregarRelatorios();
        }

        /// <summary>
        /// Lista de transações filtradas
        /// </summary>
        public ObservableCollection<Transacao> Transacoes
        {
            get => _transacoes;
            set => SetProperty(ref _transacoes, value);
        }

        /// <summary>
        /// Total de receitas no período
        /// </summary>
        public decimal TotalReceitas
        {
            get => _totalReceitas;
            set => SetProperty(ref _totalReceitas, value);
        }

        /// <summary>
        /// Total de despesas no período
        /// </summary>
        public decimal TotalDespesas
        {
            get => _totalDespesas;
            set => SetProperty(ref _totalDespesas, value);
        }

        /// <summary>
        /// Saldo no período (receitas - despesas)
        /// </summary>
        public decimal Saldo
        {
            get => _saldo;
            set => SetProperty(ref _saldo, value);
        }

        /// <summary>
        /// Data de início do filtro
        /// </summary>
        public DateTime DataInicio
        {
            get => _dataInicio;
            set => SetProperty(ref _dataInicio, value);
        }

        /// <summary>
        /// Data de fim do filtro
        /// </summary>
        public DateTime DataFim
        {
            get => _dataFim;
            set => SetProperty(ref _dataFim, value);
        }

        /// <summary>
        /// Categoria para filtrar
        /// </summary>
        public Categoria? CategoriaFiltro
        {
            get => _categoriaFiltro;
            set => SetProperty(ref _categoriaFiltro, value);
        }

        /// <summary>
        /// Lista de categorias para o filtro
        /// </summary>
        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        /// <summary>
        /// Relatório por categoria
        /// </summary>
        public ObservableCollection<RelatorioPorCategoria> RelatorioPorCategoria
        {
            get => _relatorioPorCategoria;
            set => SetProperty(ref _relatorioPorCategoria, value);
        }

        // Comandos
        public RelayCommand GerarRelatorioCommand { get; }
        public RelayCommand ExportarRelatorioCommand { get; }

        /// <summary>
        /// Carrega os relatórios iniciais
        /// </summary>
        public async void CarregarRelatorios()
        {
            try
            {
                // Carregar categorias para o filtro
                var categorias = await _context.Categorias
                    .Where(c => c.Ativa)
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                Categorias.Clear();
                Categorias.Add(new Categoria { Id = 0, Nome = "Todas as categorias" });
                foreach (var categoria in categorias)
                {
                    Categorias.Add(categoria);
                }

                CategoriaFiltro = Categorias.First();

                // Gerar relatório inicial
                GerarRelatorio();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar relatórios: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera o relatório com base nos filtros
        /// </summary>
        private async void GerarRelatorio()
        {
            try
            {
                // Query base para transações
                var query = _context.Transacoes
                    .Include(t => t.Categoria)
                    .Where(t => t.Data >= DataInicio && t.Data <= DataFim);

                // Aplicar filtro de categoria se selecionado
                if (CategoriaFiltro != null && CategoriaFiltro.Id > 0)
                {
                    query = query.Where(t => t.CategoriaId == CategoriaFiltro.Id);
                }

                // Carregar transações
                var transacoes = await query
                    .OrderByDescending(t => t.Data)
                    .ToListAsync();

                Transacoes.Clear();
                foreach (var transacao in transacoes)
                {
                    Transacoes.Add(transacao);
                }

                // Calcular totais usando LINQ
                var receitas = transacoes
                    .Where(t => t.Tipo == TipoTransacao.Receita)
                    .Sum(t => t.Valor);

                var despesas = transacoes
                    .Where(t => t.Tipo == TipoTransacao.Despesa)
                    .Sum(t => t.Valor);

                TotalReceitas = receitas;
                TotalDespesas = despesas;
                Saldo = receitas - despesas;

                // Gerar relatório por categoria
                GerarRelatorioPorCategoria(transacoes);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao gerar relatório: {ex.Message}");
            }
        }

        /// <summary>
        /// Gera relatório agrupado por categoria
        /// </summary>
        private void GerarRelatorioPorCategoria(IEnumerable<Transacao> transacoes)
        {
            try
            {
                var relatorio = transacoes
                    .GroupBy(t => new { t.Categoria.Nome, t.Categoria.Tipo })
                    .Select(g => new RelatorioPorCategoria
                    {
                        Categoria = g.Key.Nome,
                        Tipo = g.Key.Tipo,
                        Total = g.Sum(t => t.Valor),
                        Quantidade = g.Count(),
                        Media = g.Average(t => t.Valor)
                    })
                    .OrderByDescending(r => r.Total)
                    .ToList();

                RelatorioPorCategoria.Clear();
                foreach (var item in relatorio)
                {
                    RelatorioPorCategoria.Add(item);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao gerar relatório por categoria: {ex.Message}");
            }
        }

        /// <summary>
        /// Exporta o relatório (implementação básica)
        /// </summary>
        private void ExportarRelatorio()
        {
            try
            {
                // Em uma implementação real, você exportaria para CSV, Excel, etc.
                var mensagem = $"Relatório de {DataInicio:dd/MM/yyyy} a {DataFim:dd/MM/yyyy}\n" +
                              $"Total Receitas: {TotalReceitas:C}\n" +
                              $"Total Despesas: {TotalDespesas:C}\n" +
                              $"Saldo: {Saldo:C}\n" +
                              $"Quantidade de transações: {Transacoes.Count}";

                System.Windows.MessageBox.Show(mensagem, "Relatório Exportado", 
                    System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao exportar relatório: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Classe para representar dados do relatório por categoria
    /// </summary>
    public class RelatorioPorCategoria
    {
        public string Categoria { get; set; } = string.Empty;
        public TipoTransacao Tipo { get; set; }
        public decimal Total { get; set; }
        public int Quantidade { get; set; }
        public decimal Media { get; set; }
    }
} 