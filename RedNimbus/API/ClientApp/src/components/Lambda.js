import React from 'react';

class Lambda extends React.Component {

    constructor(props) {
        super(props);
    }

    render() {
        return (
            <div style={{ backgroundColor: 'red' }} className="card " style={{
                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'
            }}>
                <img style={{ height: '100px', width: '100px' }} src="https://icon-library.net//images/lambda-icon/lambda-icon-22.jpg" className="card-img-top" alt="..." />
                <div className="card-body">
                    <p style={{ textAlign: 'center' }} className="card-text">{this.props.name}</p>
                </div>
            </div>
        );
    }
}

export default Lambda;