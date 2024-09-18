ALTER SESSION SET NLS_LANGUAGE= 'AMERICAN' NLS_TERRITORY= 'AMERICA' NLS_CURRENCY= '$' NLS_ISO_CURRENCY= 'AMERICA' NLS_NUMERIC_CHARACTERS= '.,'
NLS_CALENDAR= 'GREGORIAN' NLS_DATE_FORMAT= 'DD-MON-RR' NLS_DATE_LANGUAGE= 'AMERICAN' NLS_SORT= 'BINARY';

CREATE TABLE bdt_receptionist_account (
    account_id NUMERIC(2),
    username   VARCHAR2(16) NOT NULL,
    password   VARCHAR2(64) NOT NULL
);

ALTER TABLE bdt_receptionist_account ADD CONSTRAINT bdt_receptionist_account_pk PRIMARY KEY ( account_id );

ALTER TABLE bdt_receptionist_account ADD CONSTRAINT bdt_receptionist_account_un UNIQUE ( username );

ALTER TABLE bdt_receptionist_account
    ADD CONSTRAINT bdt_recep_acc_username_ck CHECK ( REGEXP_LIKE(username, '^[a-zA-Z0-9._]{6,}$') );

ALTER TABLE bdt_receptionist_account
    ADD CONSTRAINT bdt_recep_acc_password_ck CHECK ( LENGTH(password) >= 8 );

CREATE TABLE bdt_receptionist_details (
    receptionist_id NUMERIC(2),
    first_name      VARCHAR2(16) NOT NULL,
    last_name       VARCHAR2(16) NOT NULL,
    email           VARCHAR2(32) NOT NULL,
    address         VARCHAR2(125) NOT NULL,
    phone_number    CHAR(10) NOT NULL,
    hire_date       DATE NOT NULL
);

ALTER TABLE bdt_receptionist_details ADD CONSTRAINT bdt_receptionist_details_pk PRIMARY KEY ( receptionist_id );

ALTER TABLE bdt_receptionist_details
    ADD CONSTRAINT bdt_receptionist_details_fk FOREIGN KEY ( receptionist_id )
        REFERENCES bdt_receptionist_account ( account_id );

ALTER TABLE bdt_receptionist_details
    ADD CONSTRAINT bdt_recep_det_first_name_ck CHECK ( REGEXP_LIKE(first_name, '^[A-Za-z]+$') );

ALTER TABLE bdt_receptionist_details
    ADD CONSTRAINT bdt_recep_det_last_name_ck CHECK ( REGEXP_LIKE(last_name, '^[A-Za-z]+$') );   

ALTER TABLE bdt_receptionist_details
    ADD CONSTRAINT bdt_recep_det_email_ck CHECK ( REGEXP_LIKE(email, '^[a-zA-Z0-9._]{3,}@[a-zA-Z]{2,}\.[a-zA-Z]{2,}$') );

ALTER TABLE bdt_receptionist_details
    ADD CONSTRAINT bdt_recep_det_phone_number_ck CHECK ( REGEXP_LIKE(phone_number, '^[0-9]{10}$') );

CREATE SEQUENCE SEQ_RECEPTIONIST_ID
  START WITH 1
  INCREMENT BY 1
  MINVALUE 1
  MAXVALUE 99;

CREATE TABLE bdt_service (
    service_id   NUMERIC(2),
    service_name VARCHAR2(16) NOT NULL,
    service_cost NUMERIC(6, 2) NOT NULL
);

INSERT ALL
INTO bdt_service VALUES (1, 'Fitness', 139.99)
INTO bdt_service VALUES (2, 'Cardio', 99.99)
INTO bdt_service VALUES (3, 'Spa', 199.99)
INTO bdt_service VALUES (4, 'Swimming', 399.99)
SELECT *
FROM DUAL;

ALTER TABLE bdt_service ADD CONSTRAINT bdt_service_pk PRIMARY KEY ( service_id );

CREATE TABLE bdt_membership (
    membership_id       NUMERIC(3),
    first_name          VARCHAR2(16) NOT NULL,
    last_name           VARCHAR2(16) NOT NULL,
    id_serial_number    CHAR(8) NOT NULL,
    address              VARCHAR2(125) NOT NULL,
    phone_number        CHAR(10) NOT NULL,
    services_count      NUMERIC(1) NOT NULL,
    receptionist_id     NUMERIC(3) NOT NULL
);

ALTER TABLE bdt_membership ADD CONSTRAINT bdt_membership_pk PRIMARY KEY ( membership_id );

ALTER TABLE bdt_membership
    ADD CONSTRAINT bdt_membership_fk FOREIGN KEY ( receptionist_id )
        REFERENCES bdt_receptionist_account ( account_id );

ALTER TABLE bdt_membership
    ADD CONSTRAINT bdt_memb_first_name_ck CHECK ( REGEXP_LIKE(first_name, '^[A-Za-z]+$') );

ALTER TABLE bdt_membership
    ADD CONSTRAINT bdt_memb_last_name_ck CHECK ( REGEXP_LIKE(last_name, '^[A-Za-z]+$') );

ALTER TABLE bdt_membership ADD CONSTRAINT bdt_memb_id_ser_num_ck CHECK ( REGEXP_LIKE(id_serial_number, '^[A-Z]{2}\d{6}$'));

ALTER TABLE bdt_membership ADD CONSTRAINT bdt_memb_id_ser_num_un UNIQUE ( id_serial_number );

ALTER TABLE bdt_membership ADD CONSTRAINT bdt_memb_services_count_ck CHECK ( services_count <= 3 );

CREATE SEQUENCE SEQ_MEMBERSHIP_ID
  START WITH 1
  INCREMENT BY 1
  MINVALUE 1
  MAXVALUE 999;

CREATE TABLE bdt_membership_service (
    membership_id    NUMERIC(3),
    service_id       NUMERIC(2),
    enroll_date      DATE NOT NULL,
    service_duration NUMERIC(2) NOT NULL
);

ALTER TABLE bdt_membership_service ADD CONSTRAINT bdt_membership_service_pk PRIMARY KEY ( membership_id,
                                                                                          service_id );

ALTER TABLE bdt_membership_service
    ADD CONSTRAINT bdt_memb_serv_memb_id_fk FOREIGN KEY ( membership_id )
        REFERENCES bdt_membership ( membership_id );

ALTER TABLE bdt_membership_service
    ADD CONSTRAINT bdt_memb_serv_serv_id_fk FOREIGN KEY ( service_id )
        REFERENCES bdt_service ( service_id );

ALTER TABLE bdt_membership_service ADD CONSTRAINT bdt_service_duration_check CHECK ( service_duration > 0 AND service_duration <= 24 );

INSERT INTO bdt_membership values (
    SEQ_MEMBERSHIP_ID.nextval,
    'Dragos',
    'Gherasim',
    'ZC123456',
    'Str. Hawaii, Nr.7',
    '0751234567',
    3,
    1 );
    
INSERT INTO bdt_membership_service VALUES (
    SEQ_MEMBERSHIP_ID.currval,
    1,
    '05-JAN-24',
    2
);

INSERT INTO bdt_membership_service VALUES (
    SEQ_MEMBERSHIP_ID.currval,
    2,
    '05-JAN-24',
    1
);

INSERT INTO bdt_membership values (
    SEQ_MEMBERSHIP_ID.nextval,
    'Daniel',
    'Roca',
    'ZX123456',
    'Str. Babanei, Nr.1',
    '0757654321',
    2,
    1 );

INSERT INTO bdt_membership_service VALUES (
    SEQ_MEMBERSHIP_ID.currval,
    3,
    '05-JAN-24',
    1
);

INSERT INTO bdt_membership_service VALUES (
    SEQ_MEMBERSHIP_ID.currval,
    2,
    '06-JAN-24',
    3
);

INSERT INTO bdt_membership values (
    SEQ_MEMBERSHIP_ID.nextval,
    'Elisabetta',
    'Norses',
    'AB112233',
    'Str. Mioritei, Nr.3',
    '0798765432',
    1,
    1 );
    
INSERT INTO bdt_membership_service VALUES (
    SEQ_MEMBERSHIP_ID.currval,
    1,
    '07-JAN-24',
    6
);

commit;