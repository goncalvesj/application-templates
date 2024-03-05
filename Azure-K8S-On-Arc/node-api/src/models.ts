export interface Model {
    name: string;
    age: number;
    location: string;
    date: Date;
}

export interface Config {
    PORT_REDIS: number;
    HOST_REDIS: string;    
    PASSWORD_REDIS: string;
    APP_ENVIRONMENT: string;
}