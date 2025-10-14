import { useState } from 'react';
import { FiltrosJogadores as Filtros } from '@/types';
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Card } from './ui/card';
import { Search, X, ExternalLink, Brain, Sparkles } from 'lucide-react';

interface FiltrosJogadoresProps {
  filtros: Filtros;
  onChange: (filtros: Filtros) => void;
  onFaceitSearch?: (nickname: string) => void;
  onAISearch?: (consulta: string) => void;
}

type TipoBusca = 'local' | 'faceit' | 'ia';

const FiltrosJogadores = ({ filtros, onChange, onFaceitSearch, onAISearch }: FiltrosJogadoresProps) => {
  const [buscaLocal, setBuscaLocal] = useState(filtros.q || '');
  const [buscaUnificada, setBuscaUnificada] = useState('');
  const [tipoBusca, setTipoBusca] = useState<TipoBusca>('local');

  const handleBusca = () => {
    if (tipoBusca === 'local') {
      onChange({ ...filtros, q: buscaUnificada, page: 1 });
    } else if (tipoBusca === 'faceit' && onFaceitSearch && buscaUnificada.trim()) {
      onFaceitSearch(buscaUnificada.trim());
    } else if (tipoBusca === 'ia' && onAISearch && buscaUnificada.trim()) {
      onAISearch(buscaUnificada.trim());
    }
  };

  const handleLimpar = () => {
    setBuscaLocal('');
    setBuscaUnificada('');
    setTipoBusca('local');
    onChange({ page: 1, pageSize: filtros.pageSize });
  };

  const getPlaceholder = () => {
    switch (tipoBusca) {
      case 'faceit':
        return 'Buscar na FACEIT (ex: s1mple, coldzera)...';
      case 'ia':
        return 'Buscar com IA (ex: "melhor AWP brasileiro", "jogador entry agressivo")...';
      default:
        return 'Buscar por apelido...';
    }
  };

  const getIcon = () => {
    switch (tipoBusca) {
      case 'faceit':
        return <ExternalLink className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />;
      case 'ia':
        return <Brain className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />;
      default:
        return <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />;
    }
  };

  const getButtonText = () => {
    switch (tipoBusca) {
      case 'faceit':
        return 'FACEIT';
      case 'ia':
        return 'IA';
      default:
        return 'Buscar';
    }
  };

  const getButtonVariant = () => {
    switch (tipoBusca) {
      case 'faceit':
        return 'outline';
      case 'ia':
        return 'default';
      default:
        return 'default';
    }
  };

  const getButtonClassName = () => {
    if (tipoBusca === 'ia') {
      return 'bg-gradient-to-r from-purple-500 to-pink-500 text-white hover:from-purple-600 hover:to-pink-600';
    }
    return '';
  };

  return (
    <Card className="p-4 space-y-4">
      {/* Busca Unificada */}
      <div className="space-y-3">
        <div className="flex gap-2">
          <div className="flex-1 relative">
            {getIcon()}
            <Input
              placeholder={getPlaceholder()}
              value={buscaUnificada}
              onChange={(e) => setBuscaUnificada(e.target.value)}
              onKeyDown={(e) => e.key === 'Enter' && handleBusca()}
              className="pl-9"
            />
          </div>
          <Button 
            onClick={handleBusca}
            variant={getButtonVariant()}
            disabled={!buscaUnificada.trim()}
            className={getButtonClassName()}
          >
            {tipoBusca === 'ia' && <Sparkles className="h-4 w-4 mr-2" />}
            {tipoBusca === 'faceit' && <ExternalLink className="h-4 w-4 mr-2" />}
            {getButtonText()}
          </Button>
        </div>
        
        {/* Seletor de Tipo de Busca */}
        <Select value={tipoBusca} onValueChange={(value) => setTipoBusca(value as TipoBusca)}>
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Selecione o tipo de busca" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="local">
              <div className="flex items-center">
                <Search className="h-4 w-4 mr-2" />
                Busca Local
              </div>
            </SelectItem>
            <SelectItem value="faceit">
              <div className="flex items-center">
                <ExternalLink className="h-4 w-4 mr-2" />
                FACEIT
              </div>
            </SelectItem>
            <SelectItem value="ia">
              <div className="flex items-center">
                <Brain className="h-4 w-4 mr-2" />
                IA
              </div>
            </SelectItem>
          </SelectContent>
        </Select>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-3">
        <Select
          value={filtros.status || ''}
          onValueChange={(value) => onChange({ ...filtros, status: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Status" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="Profissional">Profissional</SelectItem>
            <SelectItem value="Aposentado">Aposentado</SelectItem>
            <SelectItem value="Amador">Amador</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={filtros.disp || ''}
          onValueChange={(value) => onChange({ ...filtros, disp: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Disponibilidade" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="Livre">Livre</SelectItem>
            <SelectItem value="EmTime">Em Time</SelectItem>
            <SelectItem value="Teste">Em Teste</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={filtros.funcao || ''}
          onValueChange={(value) => onChange({ ...filtros, funcao: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Função" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="Entry">Entry</SelectItem>
            <SelectItem value="Suporte">Suporte</SelectItem>
            <SelectItem value="Awp">Awp</SelectItem>
            <SelectItem value="Igl">IGL</SelectItem>
            <SelectItem value="Lurker">Lurker</SelectItem>
          </SelectContent>
        </Select>

        <Select
          value={filtros.ordenar || ''}
          onValueChange={(value) => onChange({ ...filtros, ordenar: value as any, page: 1 })}
        >
          <SelectTrigger>
            <SelectValue placeholder="Ordenar por" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value="rating_desc">Rating (Maior)</SelectItem>
            <SelectItem value="valor_desc">Valor (Maior)</SelectItem>
            <SelectItem value="apelido_asc">Nome (A-Z)</SelectItem>
          </SelectContent>
        </Select>
      </div>

      {(filtros.q || filtros.status || filtros.disp || filtros.funcao || filtros.ordenar || buscaUnificada) && (
        <Button variant="outline" onClick={handleLimpar} className="w-full">
          <X className="h-4 w-4 mr-2" />
          Limpar Filtros
        </Button>
      )}
    </Card>
  );
};

export default FiltrosJogadores;
