using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using ACPF.Core.Models;
using ACPF.Core.Enums;
using ACPF.DataAccess;

namespace ACPF.UI.ViewModels
{
    /// <summary>
    /// ViewModel para gerenciamento de transações
    /// </summary>
    public class TransacaoViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Transacao> _transacoes;
        private ObservableCollection<Categoria> _categorias;
        private Transacao? _transacaoSelecionada;
        private string _descricao = string.Empty;
        private decimal _valor;
        private DateTime _data = DateTime.Now;
        private TipoTransacao _tipo = TipoTransacao.Despesa;
        private Categoria? _categoriaSelecionada;
        private string _observacoes = string.Empty;
        private bool _isEditando;

        public TransacaoViewModel(AppDbContext context)
        {
            _context = context;
            _transacoes = new ObservableCollection<Transacao>();
            _categorias = new ObservableCollection<Categoria>();

            // Inicializar comandos
            AdicionarTransacaoCommand = new RelayCommand(AdicionarTransacao, PodeAdicionarTransacao);
            EditarTransacaoCommand = new RelayCommand(EditarTransacao, PodeEditarTransacao);
            ExcluirTransacaoCommand = new RelayCommand(ExcluirTransacao, PodeExcluirTransacao);
            SalvarTransacaoCommand = new RelayCommand(SalvarTransacao, PodeSalvarTransacao);
            CancelarEdicaoCommand = new RelayCommand(CancelarEdicao);
            LimparFormularioCommand = new RelayCommand(LimparFormulario);

            // Carregar dados iniciais
            CarregarTransacoes();
            CarregarCategorias();
        }

        /// <summary>
        /// Lista de transações
        /// </summary>
        public ObservableCollection<Transacao> Transacoes
        {
            get => _transacoes;
            set => SetProperty(ref _transacoes, value);
        }

        /// <summary>
        /// Lista de categorias para o ComboBox
        /// </summary>
        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        /// <summary>
        /// Transação selecionada na lista
        /// </summary>
        public Transacao? TransacaoSelecionada
        {
            get => _transacaoSelecionada;
            set
            {
                SetProperty(ref _transacaoSelecionada, value);
                if (value != null)
                {
                    CarregarTransacaoParaEdicao(value);
                }
            }
        }

        /// <summary>
        /// Descrição da transação
        /// </summary>
        public string Descricao
        {
            get => _descricao;
            set
            {
                SetProperty(ref _descricao, value);
                AdicionarTransacaoCommand.RaiseCanExecuteChanged();
                SalvarTransacaoCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Valor da transação
        /// </summary>
        public decimal Valor
        {
            get => _valor;
            set
            {
                SetProperty(ref _valor, value);
                AdicionarTransacaoCommand.RaiseCanExecuteChanged();
                SalvarTransacaoCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Data da transação
        /// </summary>
        public DateTime Data
        {
            get => _data;
            set => SetProperty(ref _data, value);
        }

        /// <summary>
        /// Tipo da transação
        /// </summary>
        public TipoTransacao Tipo
        {
            get => _tipo;
            set
            {
                SetProperty(ref _tipo, value);
                FiltrarCategoriasPorTipo();
            }
        }

        /// <summary>
        /// Categoria selecionada
        /// </summary>
        public Categoria? CategoriaSelecionada
        {
            get => _categoriaSelecionada;
            set
            {
                SetProperty(ref _categoriaSelecionada, value);
                AdicionarTransacaoCommand.RaiseCanExecuteChanged();
                SalvarTransacaoCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Observações da transação
        /// </summary>
        public string Observacoes
        {
            get => _observacoes;
            set => SetProperty(ref _observacoes, value);
        }

        /// <summary>
        /// Indica se está em modo de edição
        /// </summary>
        public bool IsEditando
        {
            get => _isEditando;
            set => SetProperty(ref _isEditando, value);
        }

        // Comandos
        public RelayCommand AdicionarTransacaoCommand { get; }
        public RelayCommand EditarTransacaoCommand { get; }
        public RelayCommand ExcluirTransacaoCommand { get; }
        public RelayCommand SalvarTransacaoCommand { get; }
        public RelayCommand CancelarEdicaoCommand { get; }
        public RelayCommand LimparFormularioCommand { get; }

        /// <summary>
        /// Carrega as transações do banco de dados
        /// </summary>
        public async void CarregarTransacoes()
        {
            try
            {
                var transacoes = await _context.Transacoes
                    .Include(t => t.Categoria)
                    .OrderByDescending(t => t.Data)
                    .ToListAsync();

                Transacoes.Clear();
                foreach (var transacao in transacoes)
                {
                    Transacoes.Add(transacao);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar transações: {ex.Message}");
            }
        }

        /// <summary>
        /// Carrega as categorias do banco de dados
        /// </summary>
        public async void CarregarCategorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .Where(c => c.Ativa)
                    .OrderBy(c => c.Nome)
                    .ToListAsync();

                Categorias.Clear();
                foreach (var categoria in categorias)
                {
                    Categorias.Add(categoria);
                }

                FiltrarCategoriasPorTipo();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
            }
        }

        /// <summary>
        /// Filtra as categorias pelo tipo selecionado
        /// </summary>
        private void FiltrarCategoriasPorTipo()
        {
            var categoriasFiltradas = Categorias.Where(c => c.Tipo == Tipo).ToList();
            // Em uma implementação mais robusta, você teria uma coleção separada para categorias filtradas
        }

        /// <summary>
        /// Verifica se pode adicionar uma transação
        /// </summary>
        private bool PodeAdicionarTransacao()
        {
            return !IsEditando && 
                   !string.IsNullOrWhiteSpace(Descricao) && 
                   Valor > 0 && 
                   CategoriaSelecionada != null;
        }

        /// <summary>
        /// Verifica se pode editar uma transação
        /// </summary>
        private bool PodeEditarTransacao()
        {
            return TransacaoSelecionada != null && !IsEditando;
        }

        /// <summary>
        /// Verifica se pode excluir uma transação
        /// </summary>
        private bool PodeExcluirTransacao()
        {
            return TransacaoSelecionada != null && !IsEditando;
        }

        /// <summary>
        /// Verifica se pode salvar uma transação
        /// </summary>
        private bool PodeSalvarTransacao()
        {
            return IsEditando && 
                   !string.IsNullOrWhiteSpace(Descricao) && 
                   Valor > 0 && 
                   CategoriaSelecionada != null;
        }

        /// <summary>
        /// Adiciona uma nova transação
        /// </summary>
        private async void AdicionarTransacao()
        {
            try
            {
                var transacao = new Transacao
                {
                    Descricao = Descricao,
                    Valor = Valor,
                    Data = Data,
                    Tipo = Tipo,
                    CategoriaId = CategoriaSelecionada!.Id,
                    Observacoes = Observacoes,
                    DataCriacao = DateTime.Now,
                    DataModificacao = DateTime.Now
                };

                _context.Transacoes.Add(transacao);
                await _context.SaveChangesAsync();

                Transacoes.Insert(0, transacao);
                LimparFormulario();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar transação: {ex.Message}");
            }
        }

        /// <summary>
        /// Inicia a edição de uma transação
        /// </summary>
        private void EditarTransacao()
        {
            if (TransacaoSelecionada != null)
            {
                IsEditando = true;
                CarregarTransacaoParaEdicao(TransacaoSelecionada);
            }
        }

        /// <summary>
        /// Exclui uma transação
        /// </summary>
        private async void ExcluirTransacao()
        {
            if (TransacaoSelecionada != null)
            {
                try
                {
                    _context.Transacoes.Remove(TransacaoSelecionada);
                    await _context.SaveChangesAsync();

                    Transacoes.Remove(TransacaoSelecionada);
                    LimparFormulario();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao excluir transação: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Salva as alterações de uma transação
        /// </summary>
        private async void SalvarTransacao()
        {
            if (TransacaoSelecionada != null)
            {
                try
                {
                    TransacaoSelecionada.Descricao = Descricao;
                    TransacaoSelecionada.Valor = Valor;
                    TransacaoSelecionada.Data = Data;
                    TransacaoSelecionada.Tipo = Tipo;
                    TransacaoSelecionada.CategoriaId = CategoriaSelecionada!.Id;
                    TransacaoSelecionada.Observacoes = Observacoes;
                    TransacaoSelecionada.DataModificacao = DateTime.Now;

                    await _context.SaveChangesAsync();
                    
                    IsEditando = false;
                    LimparFormulario();
                    CarregarTransacoes(); // Recarregar para atualizar a lista
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao salvar transação: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Cancela a edição
        /// </summary>
        private void CancelarEdicao()
        {
            IsEditando = false;
            LimparFormulario();
        }

        /// <summary>
        /// Carrega uma transação para edição
        /// </summary>
        private void CarregarTransacaoParaEdicao(Transacao transacao)
        {
            Descricao = transacao.Descricao;
            Valor = transacao.Valor;
            Data = transacao.Data;
            Tipo = transacao.Tipo;
            CategoriaSelecionada = transacao.Categoria;
            Observacoes = transacao.Observacoes ?? string.Empty;
        }

        /// <summary>
        /// Limpa o formulário
        /// </summary>
        private void LimparFormulario()
        {
            Descricao = string.Empty;
            Valor = 0;
            Data = DateTime.Now;
            Tipo = TipoTransacao.Despesa;
            CategoriaSelecionada = null;
            Observacoes = string.Empty;
            TransacaoSelecionada = null;
            IsEditando = false;
        }
    }
} 