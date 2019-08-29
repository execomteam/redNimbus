import React from 'react';
import './css/Bucket.css'
import axios from 'axios';

class Bucket extends React.Component
{
    constructor(props) {
        super(props);

        this.state = {
            buckets = null
        }

        const options = {
            headers: { 'token': localStorage.getItem("token")}
        };

        axios.get("http://localhost:65001/api/bucket", options).then(
            (resp) => this.onSuccessHandler(response),
            (resp) => this.onErrorHandler(response)
        );
    }

    onErrorHandler(response){
        //will see
    }

    onSuccessHandler(resp){
        this.setState({
            bucket: resp.data
        });
    }

    render() {
        return (
            <div className="container">
                <div className="row">
                    <div className="col-md-2">
                        <button style={{marginBottom: '20px'}} className="btn btn-danger btn-sm">Crate new</button>
                        <ul className="nav flex-column">
                            <li class="nav-item">
                                <a className="nav-link active" href="#">My Bucket</a>
                            </li>
                            <li className="nav-item">
                                <a className="nav-link" href="#">Shared with me</a>
                            </li>
                        </ul>                      
                    </div>
                    <div className="col-md-10">   
                        <br />
                        <div class="card-group">
                            <div style={{ backgroundColor: 'red' }} class="card " style={{
                                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'}}>                               
                                <img style={{ height: '100px', width: '100px' }} src="https://freeiconshop.com/wp-content/uploads/edd/folder-outline-filled.png" class="card-img-top" alt="..." />
                                <div class="card-body">
                                    <p style={{ textAlign: 'center' }} class="card-text">name</p>
                                </div>
                            </div>
                            
                            <div style={{ backgroundColor: 'red' }} class="card " style={{
                                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'}}>
                                <img style={{ height: '100px', width: '100px' }} src="https://freeiconshop.com/wp-content/uploads/edd/folder-outline-filled.png" class="card-img-top" alt="..." />
                                <div class="card-body">
                                    <p style={{ textAlign: 'center' }} class="card-text">name</p>
                                </div>
                            </div>

                            <div style={{ backgroundColor: 'red' }} class="card " style={{
                                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'}}>
                                <img style={{ height: '100px', width: '100px' }} src="https://freeiconshop.com/wp-content/uploads/edd/folder-outline-filled.png" class="card-img-top" alt="..." />
                                <div class="card-body">
                                    <p style={{ textAlign: 'center' }} class="card-text">name</p>
                                </div>
                            </div>
                        </div>

                        <hr/>

                        <div class="card-group">
                            <div style={{ backgroundColor: 'red' }} class="card " style={{
                                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'}}>
                                <img style={{ height: '100px', width: '100px' }} src="https://cdn2.iconfinder.com/data/icons/55-files-and-documents/512/Icon_17-512.png" class="card-img-top" alt="..." />
                                <div class="card-body">
                                    <p style={{ textAlign: 'center' }} class="card-text">name</p>
                                </div>
                            </div>
                            <div style={{ backgroundColor: 'red' }} class="card " style={{
                                width: '100', height: '120px', display: 'inline-block', justifyContent: 'center',
                                alignItems: 'center', marginLeft: '50px', marginRight: '50px', marginTop: '50px', marginBottom: '50px'}}>

                                <img style={{ height: '100px', width: '100px' }} src="https://cdn2.iconfinder.com/data/icons/55-files-and-documents/512/Icon_17-512.png" class="card-img-top" alt="..." />
                                <div class="card-body">
                                    <p style={{ textAlign: 'center' }} class="card-text">name</p>
                                </div>
                            </div>
                            
                        </div>
                    </div>
                </div>
            </div>
        );
    }
}

export default Bucket;