import { useState } from 'react';

interface PlayerDefaultImageProps {
  src?: string;
  alt: string;
  className?: string;
}

const PlayerDefaultImage = ({ src, alt, className = "" }: PlayerDefaultImageProps) => {
  const [imageError, setImageError] = useState(false);

  // Imagem padrão (silhueta de jogador)
  const defaultImage = "data:image/svg+xml;base64,PHN2ZyB3aWR0aD0iMzAwIiBoZWlnaHQ9IjMwMCIgdmlld0JveD0iMCAwIDMwMCAzMDAiIGZpbGw9Im5vbmUiIHhtbG5zPSJodHRwOi8vd3d3LnczLm9yZy8yMDAwL3N2ZyI+CjxyZWN0IHdpZHRoPSIzMDAiIGhlaWdodD0iMzAwIiBmaWxsPSIjMkEyQTI5Ii8+CjxjaXJjbGUgY3g9IjE1MCIgY3k9IjEwMCIgcj0iNDAiIGZpbGw9IiMwMDAwMDAiLz4KPHJlY3QgeD0iMTAwIiB5PSIxNDAiIHdpZHRoPSIxMDAiIGhlaWdodD0iODAiIHJ4PSI0MCIgZmlsbD0iIzAwMDAwMCIvPgo8L3N2Zz4K";

  const handleImageError = () => {
    setImageError(true);
  };

  return (
    <img
      src={imageError || !src || src.includes('placeholder') ? defaultImage : src}
      alt={alt}
      className={className}
      onError={handleImageError}
    />
  );
};

export default PlayerDefaultImage;
