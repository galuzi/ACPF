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
    /// ViewModel para gerenciamento de categorias
    /// </summary>
    public class CategoriaViewModel : BaseViewModel
    {
        private readonly AppDbContext _context;
        private ObservableCollection<Categoria> _categorias;
        private Categoria? _categoriaSelecionada;
        private string _nome = string.Empty;
        private string _descricao = string.Empty;
        private TipoTransacao _tipo = TipoTransacao.Despesa;
        private string _cor = "#007ACC";
        private bool _isEditando;

        public CategoriaViewModel(AppDbContext context)
        {
            _context = context;
            _categorias = new ObservableCollection<Categoria>();

            // Inicializar comandos
            AdicionarCategoriaCommand = new RelayCommand(AdicionarCategoria, PodeAdicionarCategoria);
            EditarCategoriaCommand = new RelayCommand(EditarCategoria, PodeEditarCategoria);
            ExcluirCategoriaCommand = new RelayCommand(ExcluirCategoria, PodeExcluirCategoria);
            SalvarCategoriaCommand = new RelayCommand(SalvarCategoria, PodeSalvarCategoria);
            CancelarEdicaoCommand = new RelayCommand(CancelarEdicao);
            LimparFormularioCommand = new RelayCommand(LimparFormulario);

            // Carregar dados iniciais
            CarregarCategorias();
        }

        /// <summary>
        /// Lista de categorias
        /// </summary>
        public ObservableCollection<Categoria> Categorias
        {
            get => _categorias;
            set => SetProperty(ref _categorias, value);
        }

        /// <summary>
        /// Categoria selecionada na lista
        /// </summary>
        public Categoria? CategoriaSelecionada
        {
            get => _categoriaSelecionada;
            set
            {
                SetProperty(ref _categoriaSelecionada, value);
                if (value != null)
                {
                    CarregarCategoriaParaEdicao(value);
                }
            }
        }

        /// <summary>
        /// Nome da categoria
        /// </summary>
        public string Nome
        {
            get => _nome;
            set
            {
                SetProperty(ref _nome, value);
                AdicionarCategoriaCommand.RaiseCanExecuteChanged();
                SalvarCategoriaCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Descrição da categoria
        /// </summary>
        public string Descricao
        {
            get => _descricao;
            set => SetProperty(ref _descricao, value);
        }

        /// <summary>
        /// Tipo da categoria
        /// </summary>
        public TipoTransacao Tipo
        {
            get => _tipo;
            set => SetProperty(ref _tipo, value);
        }

        /// <summary>
        /// Cor da categoria
        /// </summary>
        public string Cor
        {
            get => _cor;
            set => SetProperty(ref _cor, value);
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
        public RelayCommand AdicionarCategoriaCommand { get; }
        public RelayCommand EditarCategoriaCommand { get; }
        public RelayCommand ExcluirCategoriaCommand { get; }
        public RelayCommand SalvarCategoriaCommand { get; }
        public RelayCommand CancelarEdicaoCommand { get; }
        public RelayCommand LimparFormularioCommand { get; }

        /// <summary>
        /// Carrega as categorias do banco de dados
        /// </summary>
        public async void CarregarCategorias()
        {
            try
            {
                var categorias = await _context.Categorias
                    .OrderBy(c => c.Tipo)
                    .ThenBy(c => c.Nome)
                    .ToListAsync();

                Categorias.Clear();
                foreach (var categoria in categorias)
                {
                    Categorias.Add(categoria);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar categorias: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica se pode adicionar uma categoria
        /// </summary>
        private bool PodeAdicionarCategoria()
        {
            return !IsEditando && !string.IsNullOrWhiteSpace(Nome);
        }

        /// <summary>
        /// Verifica se pode editar uma categoria
        /// </summary>
        private bool PodeEditarCategoria()
        {
            return CategoriaSelecionada != null && !IsEditando;
        }

        /// <summary>
        /// Verifica se pode excluir uma categoria
        /// </summary>
        private bool PodeExcluirCategoria()
        {
            return CategoriaSelecionada != null && !IsEditando;
        }

        /// <summary>
        /// Verifica se pode salvar uma categoria
        /// </summary>
        private bool PodeSalvarCategoria()
        {
            return IsEditando && !string.IsNullOrWhiteSpace(Nome);
        }

        /// <summary>
        /// Adiciona uma nova categoria
        /// </summary>
        private async void AdicionarCategoria()
        {
            try
            {
                var categoria = new Categoria
                {
                    Nome = Nome,
                    Descricao = Descricao,
                    Tipo = Tipo,
                    Cor = Cor,
                    DataCriacao = DateTime.Now,
                    Ativa = true
                };

                _context.Categorias.Add(categoria);
                await _context.SaveChangesAsync();

                Categorias.Add(categoria);
                LimparFormulario();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao adicionar categoria: {ex.Message}");
            }
        }

        /// <summary>
        /// Inicia a edição de uma categoria
        /// </summary>
        private void EditarCategoria()
        {
            if (CategoriaSelecionada != null)
            {
                IsEditando = true;
                CarregarCategoriaParaEdicao(CategoriaSelecionada);
            }
        }

        /// <summary>
        /// Exclui uma categoria
        /// </summary>
        private async void ExcluirCategoria()
        {
            if (CategoriaSelecionada != null)
            {
                try
                {
                    // Verificar se há transações usando esta categoria
                    var temTransacoes = await _context.Transacoes
                        .AnyAsync(t => t.CategoriaId == CategoriaSelecionada.Id);

                    if (temTransacoes)
                    {
                        // Em vez de excluir, apenas desativar
                        CategoriaSelecionada.Ativa = false;
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        // Se não há transações, pode excluir
                        _context.Categorias.Remove(CategoriaSelecionada);
                        await _context.SaveChangesAsync();
                        Categorias.Remove(CategoriaSelecionada);
                    }

                    LimparFormulario();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao excluir categoria: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Salva as alterações de uma categoria
        /// </summary>
        private async void SalvarCategoria()
        {
            if (CategoriaSelecionada != null)
            {
                try
                {
                    CategoriaSelecionada.Nome = Nome;
                    CategoriaSelecionada.Descricao = Descricao;
                    CategoriaSelecionada.Tipo = Tipo;
                    CategoriaSelecionada.Cor = Cor;

                    await _context.SaveChangesAsync();
                    
                    IsEditando = false;
                    LimparFormulario();
                    CarregarCategorias(); // Recarregar para atualizar a lista
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Erro ao salvar categoria: {ex.Message}");
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
        /// Carrega uma categoria para edição
        /// </summary>
        private void CarregarCategoriaParaEdicao(Categoria categoria)
        {
            Nome = categoria.Nome;
            Descricao = categoria.Descricao ?? string.Empty;
            Tipo = categoria.Tipo;
            Cor = categoria.Cor ?? "#007ACC";
        }

        /// <summary>
        /// Limpa o formulário
        /// </summary>
        private void LimparFormulario()
        {
            Nome = string.Empty;
            Descricao = string.Empty;
            Tipo = TipoTransacao.Despesa;
            Cor = "#007ACC";
            CategoriaSelecionada = null;
            IsEditando = false;
        }
    }
} 