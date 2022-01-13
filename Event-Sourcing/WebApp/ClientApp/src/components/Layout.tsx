import React, { FunctionComponent } from 'react';
import { NavMenu } from './NavMenu';
import { Container } from 'reactstrap';

interface ILayoutProps {}

export const Layout: FunctionComponent<ILayoutProps> = ({ children }) => {
  return (
    <div>
      <NavMenu />
      <Container>{children}</Container>
    </div>
  );
};
