import { Link } from 'react-router-dom';
import { Button } from '@/components/ui/button';
import { ArrowRight, Users, Trophy, TrendingUp } from 'lucide-react';
import heroImage from '@/assets/hero-cs2.jpg';

const Index = () => {
  return (
    <div className="min-h-screen bg-background">

      {/* Hero Section */}
      <section className="relative h-[600px] flex items-center justify-center overflow-hidden">
        <div className="absolute inset-0">
          <img 
            src={heroImage} 
            alt="Counter-Strike 2"
            className="w-full h-full object-cover"
          />
          <div className="absolute inset-0 bg-gradient-to-r from-black/80 via-black/50 to-black/80" />
        </div>

        <div className="container mx-auto px-4 relative z-10 text-center">
          <h1 className="text-5xl md:text-7xl font-bold mb-6 animate-fade-in">
            <span className="text-white">
              ES-TOP1
            </span>
          </h1>
          <p className="text-xl md:text-2xl text-white/90 mb-8 max-w-2xl mx-auto animate-fade-in">
            A plataforma definitiva para descobrir e contratar os melhores jogadores profissionais de E-Sports
          </p>
          <div className="flex gap-4 justify-center animate-fade-in">
            <Button size="lg" asChild className="group">
              <Link to="/jogadores">
                Explorar Jogadores
                <ArrowRight className="ml-2 h-5 w-5 group-hover:translate-x-1 transition-transform" />
              </Link>
            </Button>
            <Button size="lg" variant="secondary" asChild>
              <Link to="/times">Ver Times</Link>
            </Button>
          </div>
        </div>
      </section>

      {/* Stats Section */}
      <section className="py-16 bg-muted/50">
        <div className="container mx-auto px-4">
          <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
            <div className="text-center space-y-2 animate-fade-in">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-primary/10 mb-4">
                <Users className="h-8 w-8 text-primary" />
              </div>
              <h3 className="text-4xl font-bold font-mono text-primary">500+</h3>
              <p className="text-muted-foreground">Jogadores Profissionais</p>
            </div>

            <div className="text-center space-y-2 animate-fade-in">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-secondary/10 mb-4">
                <Trophy className="h-8 w-8 text-secondary" />
              </div>
              <h3 className="text-4xl font-bold font-mono text-secondary">100+</h3>
              <p className="text-muted-foreground">Times Cadastrados</p>
            </div>

            <div className="text-center space-y-2 animate-fade-in">
              <div className="inline-flex items-center justify-center w-16 h-16 rounded-full bg-success/10 mb-4">
                <TrendingUp className="h-8 w-8 text-success" />
              </div>
              <h3 className="text-4xl font-bold font-mono text-success">1000+</h3>
              <p className="text-muted-foreground">Estatísticas Registradas</p>
            </div>
          </div>
        </div>
      </section>

      {/* Features Section */}
      <section className="py-16">
        <div className="container mx-auto px-4">
          <h2 className="text-3xl md:text-4xl font-bold text-center mb-12">
            Encontre o Jogador Perfeito
          </h2>

          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
            <div className="p-6 rounded-lg border bg-card hover:shadow-lg transition-shadow">
              <div className="h-12 w-12 rounded-lg bg-primary/10 flex items-center justify-center mb-4">
                <Users className="h-6 w-6 text-primary" />
              </div>
              <h3 className="text-xl font-bold mb-2">Perfis Completos</h3>
              <p className="text-muted-foreground">
                Acesse informações detalhadas sobre cada jogador, incluindo estatísticas, histórico e valor de mercado.
              </p>
            </div>

            <div className="p-6 rounded-lg border bg-card hover:shadow-lg transition-shadow">
              <div className="h-12 w-12 rounded-lg bg-secondary/10 flex items-center justify-center mb-4">
                <TrendingUp className="h-6 w-6 text-secondary" />
              </div>
              <h3 className="text-xl font-bold mb-2">Estatísticas em Tempo Real</h3>
              <p className="text-muted-foreground">
                Acompanhe ratings, K/D e performance dos jogadores com dados sempre atualizados.
              </p>
            </div>

            <div className="p-6 rounded-lg border bg-card hover:shadow-lg transition-shadow">
              <div className="h-12 w-12 rounded-lg bg-accent/10 flex items-center justify-center mb-4">
                <Trophy className="h-6 w-6 text-accent" />
              </div>
              <h3 className="text-xl font-bold mb-2">Times Profissionais</h3>
              <p className="text-muted-foreground">
                Explore times e seus rosters completos, descubra talentos e oportunidades.
              </p>
            </div>
          </div>
        </div>
      </section>

      {/* CTA Section */}
      <section className="py-16 bg-gradient-primary">
        <div className="container mx-auto px-4 text-center">
          <h2 className="text-3xl md:text-4xl font-bold text-white mb-4">
            Pronto para começar?
          </h2>
          <p className="text-xl text-white/90 mb-8 max-w-2xl mx-auto">
            Cadastre-se agora e tenha acesso completo à maior plataforma de vendas de jogadores profissionais de CS2.
          </p>
          <div className="flex flex-col sm:flex-row gap-4 justify-center">
            <Button size="lg" variant="secondary" asChild>
              <Link to="/cadastro">
                Criar Conta Gratuita
                <ArrowRight className="ml-2 h-5 w-5" />
              </Link>
            </Button>
            <Button size="lg" variant="outline" asChild>
              <Link to="/inscricao">
                Inscrever-se como Aspirante
              </Link>
            </Button>
          </div>
        </div>
      </section>

      {/* Footer */}
      <footer className="py-8 border-t">
        <div className="container mx-auto px-4 text-center text-sm text-muted-foreground">
          <p>&copy; 2025 ES-TOP1. Todos os direitos reservados.</p>
        </div>
      </footer>
    </div>
  );
};

export default Index;
