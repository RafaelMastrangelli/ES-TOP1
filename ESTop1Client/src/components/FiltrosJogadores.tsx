import { useState } from 'react';
import { FiltrosJogadores as Filtros } from '@/types';
import { Input } from './ui/input';
import { Button } from './ui/button';
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from './ui/select';
import { Card } from './ui/card';
import { Search, X, ExternalLink } from 'lucide-react';

interface FiltrosJogadoresProps {
  filtros: Filtros;
  onChange: (filtros: Filtros) => void;
  onFaceitSearch?: (nickname: string) => void;
}

const FiltrosJogadores = ({ filtros, onChange, onFaceitSearch }: FiltrosJogadoresProps) => {
  const [buscaLocal, setBuscaLocal] = useState(filtros.q || '');
  const [buscaFaceit, setBuscaFaceit] = useState('');

  const handleBusca = () => {
    onChange({ ...filtros, q: buscaLocal, page: 1 });
  };

  const handleBuscaFaceit = () => {
    if (onFaceitSearch && buscaFaceit.trim()) {
      onFaceitSearch(buscaFaceit.trim());
    }
  };

  const handleLimpar = () => {
    setBuscaLocal('');
    setBuscaFaceit('');
    onChange({ page: 1, pageSize: filtros.pageSize });
  };

  return (
    <Card className="p-4 space-y-4">
      {/* Busca Local */}
      <div className="flex gap-2">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Buscar por apelido..."
            value={buscaLocal}
            onChange={(e) => setBuscaLocal(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleBusca()}
            className="pl-9"
          />
        </div>
        <Button onClick={handleBusca}>Buscar</Button>
      </div>

      {/* Busca FACEIT */}
      <div className="flex gap-2">
        <div className="flex-1 relative">
          <ExternalLink className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
          <Input
            placeholder="Buscar na FACEIT (ex: s1mple, coldzera)..."
            value={buscaFaceit}
            onChange={(e) => setBuscaFaceit(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && handleBuscaFaceit()}
            className="pl-9"
          />
        </div>
        <Button 
          onClick={handleBuscaFaceit}
          variant="outline"
          disabled={!buscaFaceit.trim()}
        >
          <ExternalLink className="h-4 w-4 mr-2" />
          FACEIT
        </Button>
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

      {(filtros.q || filtros.status || filtros.disp || filtros.funcao || filtros.ordenar) && (
        <Button variant="outline" onClick={handleLimpar} className="w-full">
          <X className="h-4 w-4 mr-2" />
          Limpar Filtros
        </Button>
      )}
    </Card>
  );
};

export default FiltrosJogadores;
